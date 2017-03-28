import rollup      from 'rollup'
import nodeResolve from 'rollup-plugin-node-resolve'
import commonjs    from 'rollup-plugin-commonjs';
import uglify      from 'rollup-plugin-uglify'
export default {
entry: 'app/default/main.js',
    dest: 'dist/defaultbuild.js', // output a single application bundle
sourceMap: false,
format: 'iife',
plugins: [
    nodeResolve({jsnext: true, module: true}),
    commonjs({
        include: 'node_modules/rxjs/**',
    }),
    uglify()
]
}