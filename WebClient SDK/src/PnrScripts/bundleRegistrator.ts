import { ExtensionContainer } from 'tessa/extensions';
ExtensionContainer.instance.registerBundle({
  name: 'Tessa.Extensions.js',
  buildTime: process.env.BUILD_TIME!
});