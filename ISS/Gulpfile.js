var gulp = require('gulp');
var $ = require('gulp-load-plugins')({ lazy: true });
var config = require('./gulp.config')();
var del = require('del');
var exec = require('child_process').exec;

gulp.task('vet', function () {
    log('Analysing code with TSLint....');
    return gulp.src(config.authtsSource)
    .pipe($.tslint({
        configuration: "tslint.json",
        formatter: "verbose",
        force: "true"
    }))
    .pipe($.tslint.report())
});

gulp.task('clean-defaultstyles', function () {
    log('Cleaning Default CSS....');
    var files = config.defaultappCssSource;
    return clean(files);
});

gulp.task('clean-authstyles', function () {
    log('Cleaning Auth CSS....');
    var files = config.authappCssSource;
    return clean(files);
});

gulp.task('defaultsass', ['clean-defaultstyles'], function () {
    log('Compiling SASS....');
    return gulp.src(config.defaultappSassSource)
    .pipe($.plumber())
    .pipe($.sass())
    .pipe($.autoprefixer({browsers: ['last 2 version', '> 5%']}))
    .pipe(gulp.dest(config.defaultappSassDest));
});

gulp.task('authsass', ['clean-authstyles'], function () {
    log('Compiling SASS....');
    return gulp.src(config.authappSassSource)
    .pipe($.plumber())
    .pipe($.sass())
    .pipe($.autoprefixer({ browsers: ['last 2 version', '> 5%'] }))
    .pipe(gulp.dest(config.authappSassDest));
});

gulp.task('clean-aot', function () {
    log('Cleaning AOT Compilation....');
    var files = config.aotDestination;
    return clean(files);
});

gulp.task('defaultaot', ['defaultsass'], function (done) {
    log('Performing Default AOT Compilation....');
    exec('node_modules\\.bin\\ngc -p "tsconfig-defaultaot.json"', function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        done(err);
    });
});

gulp.task('authaot', ['authsass'], function (done) {
    log('Performing Auth AOT Compilation....');
    exec('node_modules\\.bin\\ngc -p "tsconfig-authaot.json"', function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        done(err);
    });
});

gulp.task('clean-build', function () {
    log('Cleaning Default Distribution Package....');
    var files = config.rollupDestination;
    return clean(files);
});

gulp.task('builddefault', ['defaultaot'], function (done) {
    log('Creating Default Distribution Package....');
    exec('node_modules\\.bin\\rollup -c "rollup-config-default.js"', function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        done(err);
    });
});

gulp.task('buildauth', ['authaot'], function (done) {
    log('Creating Auth Distribution Package....');
    exec('node_modules\\.bin\\rollup -c "rollup-config-auth.js"', function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        done(err);
    });
});

gulp.task('build', ['clean-aot','clean-build'], function (done) {
    gulp.start('builddefault');
    gulp.start('buildauth');
    done();
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