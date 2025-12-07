const fs = require('fs');
const path = require('path');
const { withAndroidManifest, withDangerousMod } = require('@expo/config-plugins');

const NETWORK_CONFIG_CONTENT = `<?xml version="1.0" encoding="utf-8"?>
<network-security-config>
    <domain-config cleartextTrafficPermitted="true">
        <domain includeSubdomains="true">10.200.117.50</domain>
        <domain includeSubdomains="true">localhost</domain>
        <domain includeSubdomains="true">127.0.0.1</domain>
    </domain-config>
</network-security-config>
`;

const ensureManifestAttrs = (androidManifest) => {
    if (!androidManifest.manifest || !androidManifest.manifest.application?.length) {
        return androidManifest;
    }
    const app = androidManifest.manifest.application[0];
    app.$['android:usesCleartextTraffic'] = 'true';
    app.$['android:networkSecurityConfig'] = '@xml/network_security_config';
    return androidManifest;
};

const withNetworkSecurityConfig = (config) =>
    withDangerousMod(config, [
        'android',
        async (cfg) => {
            const filePath = path.join(
                cfg.modRequest.projectRoot,
                'android',
                'app',
                'src',
                'main',
                'res',
                'xml',
                'network_security_config.xml'
            );
            await fs.promises.mkdir(path.dirname(filePath), { recursive: true });
            await fs.promises.writeFile(filePath, NETWORK_CONFIG_CONTENT, 'utf8');
            return cfg;
        },
    ]);

module.exports = function withCleartext(config) {
    config = withAndroidManifest(config, async (cfg) => {
        cfg.modResults = ensureManifestAttrs(cfg.modResults);
        return cfg;
    });
    config = withNetworkSecurityConfig(config);
    return config;
};
