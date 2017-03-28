module.exports = function () {
    var config = {
        tsSource: "app/**/*.ts",
        appSassSource: "app/Styles/*.scss",
        appSassDest: "app/Styles",
        appCssSource: "app/Styles/*.css",
        aotDestination: "aot/**/*.*",
        rollupDestination: "dist/*.*"
    };

    return config
}