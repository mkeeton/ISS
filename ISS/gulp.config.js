module.exports = function () {
    var config = {
        defaulttsSource: "app/default/**/*.ts",
        defaultappSassSource: "app/default/Styles/*.scss",
        defaultappSassDest: "app/default/Styles",
        defaultappCssSource: "app/default/Styles/*.css",
        authtsSource: "app/auth/**/*.ts",
        authappSassSource: "app/auth/Styles/*.scss",
        authappSassDest: "app/auth/Styles",
        authappCssSource: "app/auth/Styles/*.css",
        aotDestination: "aot/**/*.*",
        rollupDestination: "dist/*.*"
    };

    return config
}