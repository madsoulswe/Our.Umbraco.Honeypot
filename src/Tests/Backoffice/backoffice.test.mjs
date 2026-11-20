import { strict as assert } from "node:assert";
import { readFile } from "node:fs/promises";
import path from "node:path";
import { test } from "node:test";
import vm from "node:vm";
import { fileURLToPath } from "node:url";

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "../..");
const plugin = process.env.HONEYPOT_PLUGIN_PATH ??
    path.join(root, "Our.Umbraco.Honeypot", "App_Plugins", "Our.Umbraco.Honeypot");

test("the packaged entry point registers working preview and editor elements", async () => {
    const packageManifest = JSON.parse(await readFile(path.join(plugin, "umbraco-package.json"), "utf8"));
    const entry = packageManifest.extensions.find((extension) => extension.type === "backofficeEntryPoint");
    assert.ok(entry);
    const prefix = "/App_Plugins/Our.Umbraco.Honeypot/";
    assert.ok(entry.js.startsWith(prefix));
    const source = await readFile(path.join(plugin, entry.js.slice(prefix.length)), "utf8");

    const elements = new Map();
    const context = vm.createContext({
        customElements: {
            define(name, element) {
                assert.equal(elements.has(name), false);
                elements.set(name, element);
            },
        },
    });
    const html = (strings, ...values) => String.raw({ raw: strings }, ...values);
    class PreviewBase {}
    class LitElement {}
    const modules = {
        "@umbraco-cms/backoffice/external/lit": { html },
        "@umbraco-forms/backoffice": { FormsFieldPreviewBaseElement: PreviewBase },
        "@umbraco-cms/backoffice/lit-element": { UmbLitElement: LitElement },
    };
    const entryModule = new vm.SourceTextModule(source, { context });
    await entryModule.link((specifier) => {
        assert.ok(Object.hasOwn(modules, specifier), `Unexpected import: ${specifier}`);
        const exports = modules[specifier];
        return new vm.SyntheticModule(Object.keys(exports), function () {
            for (const [name, value] of Object.entries(exports)) this.setExport(name, value);
        }, { context });
    });
    await entryModule.evaluate();

    let registered;
    entryModule.namespace.onInit({}, { registerMany: (manifests) => { registered = manifests; } });
    for (const [alias, type, tag, base] of [
        ["Honeypot.FieldPreview.Image", "formsFieldPreview", "honeypot-preview-image", PreviewBase],
        ["Honeypot.PropertyEditorUi.Image", "propertyEditorUi", "honeypot-editor-ui-image", LitElement],
    ]) {
        const manifest = registered.find((item) => item.alias === alias);
        assert.ok(manifest);
        assert.equal(manifest.type, type);
        assert.equal(manifest.element, elements.get(tag));
        const element = new manifest.element();
        assert.ok(element instanceof base);
        assert.match(element.render(), /<img width="60" height="60" src="data:image\/png;base64,/);
    }
});
