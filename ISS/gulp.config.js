module.exports = function () {
    var config = {
        defaulttsSource: "app/auth/**/*.ts",
        defaultappSassSource: "app/auth/Styles/*.scss",
        defaultappSassDest: "app/auth/Styles",
        defaultappCssSource: "app/auth/Styles/*.css",
        authtsSource: "app/auth/**/*.ts",
        authappSassSource: "app/auth/Styles/*.scss",
        authappSassDest: "app/auth/Styles",
        authappCssSource: "app/auth/Styles/*.css",
        aotDestination: "aot/**/*.*",
        rollupDestination: "dist/*.*"
    };

    return config
}