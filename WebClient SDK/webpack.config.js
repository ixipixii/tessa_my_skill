var path = require('path');
var webpack = require('webpack');
var CleanWebpackPlugin = require('clean-webpack-plugin');
// var fs = require('fs');

// const examplesPath = './examples';
// const entry = fs.readdirSync(examplesPath).reduce((s, v) => {
//   s[v] = examplesPath + '/' + v;
//   return s;
// }, {});
const outputPath = path.join(__dirname, '/wwwroot/extensions');
const ForkTsCheckerWebpackPlugin = require('fork-ts-checker-webpack-plugin');

module.exports = {
  context: path.join(__dirname),
  mode: 'development',
  devtool: 'source-map',
  entry: {
    pnrScripts: './src/PnrScripts/index.ts'
  },
  output: {
    path: outputPath,
    filename: '[name].js'
  },
  externals: [
    {
      react: 'React',
      'react-dom': 'ReactDom',
      classnames: 'classnames',
      mobx: 'mobx',
      'mobx-utils': 'mobxUtils',
      'mobx-react': 'mobxReact',
      moment: 'Moment',
      'styled-components': 'styledComponents'
    },
    function(context, request, callback) {
      if (/^common|^ui|^components|^tessa/.test(request)) {
        return callback(null, 'var tessa.' + request.match(/\w+/g).join('.'));
      }
      callback();
    }
  ],
  module: {
    rules: [
      {
        test: /.jsx?$/,
        loader: 'babel-loader',
        exclude: /node_modules/
      },
      {
        test: /\.tsx?$/,
        //loaders: ['babel-loader', 'ts-loader'],
        use: [
          { loader: 'cache-loader' },
          { loader: 'babel-loader' },
          { loader: 'ts-loader', options: { transpileOnly: true } }
        ],
        exclude: /node_modules/
      }
    ]
  },
  plugins: [
    new CleanWebpackPlugin([outputPath], {
      verbose: true
    }),
    new webpack.DefinePlugin({
      'process.env': {
        NODE_ENV: JSON.stringify('production'),
        BUILD_TIME: JSON.stringify(Date.now())
      }
    }),
    new ForkTsCheckerWebpackPlugin(),
  ],
  resolve: {
    extensions: ['.ts', '.tsx', '.js', '.jsx', '.css', '.scss'],
    modules: [path.resolve(__dirname), 'node_modules']
  }
};
