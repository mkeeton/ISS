var gulp = require('gulp');
var $ = require('gulp-load-plugins')({ lazy: true });
var config = require('./gulp.config')();
var del = require('del');
var exec = require('child_process').exec;

gulp.task('vet', function () {
    log('Analysing code with TSLint....');
    return gulp.src(config.tsSource)
    .pipe($.tslint({
        configuration: "tslint.json",
        formatter: "verbose",
        force: "true"
    }))
    .pipe($.tslint.report())
});

gulp.task('clean-styles', function () {
    log('Cleaning CSS....');
    var files = config.appCssSource;
    return clean(files);
});

gulp.task('sass', ['clean-styles'], function () {
    log('Compiling SASS....');
    return gulp.src(config.appSassSource)
    .pipe($.plumber())
    .pipe($.sass())
    .pipe($.autoprefixer({browsers: ['last 2 version', '> 5%']}))
    .pipe(gulp.dest(config.appSassDest));
});

gulp.task('clean-aot', function () {
    log('Cleaning AOT Compilation....');
    var files = config.aotDestination;
    return clean(files);
});

gulp.task('aot', ['sass', 'clean-aot'], function (done) {
    log('Performing AOT Compilation....');
    exec('node_modules\\.bin\\ngc -p "tsconfig-aot.json"', function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        done(err);
    });
});

gulp.task('clean-build', function () {
    log('Cleaning Distribution Package....');
    var files = config.rollupDestination;
    return clean(files);
});

gulp.task('build', ['aot', 'clean-build'], function (done) {
    log('Creating Distribution Package....');
    exec('node_modules\\.bin\\rollup -c "rollup-config.js"', function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        done(err);
    });
});

function clean(path) {
    log('Cleaning: ' + path);
    return del(path);
}

function log(msg) {
    if (typeof (msg) === 'object') {
        for (var item in msg) {
            if (msg.hasOwnProperty(item)) {
                $.util.log($.util.colors.blue(msg[item]));
            }
        }
    } else {
        $.util.log($.util.colors.blue(msg));
    }
}