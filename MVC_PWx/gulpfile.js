const gulp = require('gulp');
//const concat = require('gulp-concat');

const paths = {
    nodeModules: './node_modules/',
    scriptsDest: './Scripts/',
    stylesDest: './Content/'
};

gulp.task('copy:css', () => {
    const cssToCopy = [
        `${paths.nodeModules}bootstrap-select/dist/css/bootstrap-select.min.css`
    ];

    return gulp.src(cssToCopy)
        .pipe(gulp.dest(`${paths.stylesDest}`));
});

gulp.task('copy:js', () => {
    const javascriptToCopy = [
        `${paths.nodeModules}bootstrap-select/dist/js/bootstrap-select.min.js`,
        `${paths.nodeModules}jquery.redirect/jquery.redirect.js`
    ];

    return gulp.src(javascriptToCopy)
        .pipe(gulp.dest(`${paths.scriptsDest}`));
});