const gulp = require('gulp');
const minify = require('gulp-minify');
const watch = require('gulp-watch');
const del = require('del');
//const concat = require('gulp-concat');

const paths = {
    nodeModules: './node_modules/',
    scriptsDest: './Scripts/',
    stylesDest: './Content/'
};

gulp.task('copy:css', () => {
    const cssToCopy = [
        `${paths.nodeModules}bootstrap-select/dist/css/bootstrap-select.min.css`,
        `${paths.nodeModules}xbs-enjoyhint/enjoyhint.css`
    ];

    return gulp.src(cssToCopy)
        .pipe(gulp.dest(`${paths.stylesDest}`));
});

gulp.task('copy:js', () => {
    const javascriptToCopy = [
        `${paths.nodeModules}bootstrap-select/dist/js/bootstrap-select.min.js`,
        `${paths.nodeModules}jquery.redirect/jquery.redirect.js`,
        `${paths.nodeModules}xbs-enjoyhint/enjoyhint.min.js`,
        `${paths.nodeModules}xbs-enjoyhint/enjoyhint.js`,
        `${paths.nodeModules}jquery.scrollto/jquery.scrollTo.min.js`,
        `${paths.nodeModules}kinetic/kinetic.min.js`,
        `${paths.nodeModules}js-cookie/dist/js.cookie.min.js`
        /*,`${paths.nodeModules}gulp-minify/index.js`*/
    ];

    return gulp.src(javascriptToCopy)
        .pipe(gulp.dest(`${paths.scriptsDest}`));
});

gulp.task('minify:js', function () {
    return gulp.src([`${paths.scriptsDest}*.js`])
        .pipe(minify({
            ext: {
                src: '.js',
                min: '.min.js'
            },
            ignoreFiles: ['*.min.*']
        }))
        .pipe(gulp.dest(`${paths.scriptsDest}`))
});

gulp.task('delete:js', function () {
    var filesToDelete = [
        `${paths.scriptsDest}enjoyhint.min.js`,
        `${paths.scriptsDest}Site.min.js`
    ]
    return del(filesToDelete);
});