// 使用官方 npm 包的资源映射，避免依赖全局 ABP CLI。
const fs = require('node:fs');
const path = require('node:path');
const root = path.resolve(__dirname, '../samples/BookStore/src/Acme.BookStore.Web');
const modules = path.join(root, 'node_modules');
const libs = path.join(root, 'wwwroot/libs');
const started = Date.now();
let operations = 0;

function check() {
  if (++operations > 4000 || Date.now() - started > 30000) {
    throw new Error('Asset mapping exceeded 4000 operations or 30 seconds.');
  }
}

function within(file, directory) {
  const relative = path.relative(directory, file);
  if (relative.startsWith('..') || path.isAbsolute(relative)) {
    throw new Error(`Asset path escaped its root: ${file}`);
  }
}

// 当前官方包使用单目录通配符；遇到未知映射立即报错。
const packages = fs.readdirSync(path.join(modules, '@abp'), { withFileTypes: true });
if (packages.length > 60) throw new Error('ABP package count exceeded 60.');
let copied = 0;
const singlePackage = process.argv[2];
for (const pkg of packages) {
  check();
  if (singlePackage && pkg.name !== singlePackage) continue;
  if (!pkg.isDirectory()) continue;
  const mappingFile = path.join(modules, '@abp', pkg.name, 'abp.resourcemapping.js');
  if (!fs.existsSync(mappingFile)) continue;
  const config = require(mappingFile);
  if (config.aliases && Object.keys(config.aliases).length || config.clean?.length) {
    throw new Error(`Unsupported aliases/clean in ${mappingFile}`);
  }
  const mappings = Object.entries(config.mappings || {});
  if (mappings.length > 100) throw new Error('Mapping count exceeded 100.');
  for (const [source, target] of mappings) {
    check();
    if (!source.startsWith('@node_modules/') || !target.startsWith('@libs/')) {
      throw new Error(`Unknown mapping: ${source} -> ${target}`);
    }
    const pattern = path.resolve(modules, source.substring('@node_modules/'.length));
    const destination = path.resolve(libs, target.substring('@libs/'.length));
    within(pattern, modules);
    within(destination, libs);
    const sourceDir = path.dirname(pattern);
    const filePattern = path.basename(pattern);
    if (/[*?]/.test(sourceDir)) throw new Error(`Unsupported nested glob: ${source}`);
    const expression = new RegExp('^' + filePattern.replace(/[.+^${}()|[\]\\]/g, '\\$&').replace(/\*/g, '.*').replace(/\?/g, '.') + '$');
    const candidates = fs.readdirSync(sourceDir, { withFileTypes: true });
    if (candidates.length > 1000) throw new Error('Source entry count exceeded 1000.');
    const matches = candidates.filter(item => item.isFile() && expression.test(item.name));
    if (!matches.length) throw new Error(`No assets matched ${source}`);
    fs.mkdirSync(destination, { recursive: true });
    for (const item of matches) {
      check();
      fs.copyFileSync(path.join(sourceDir, item.name), path.join(destination, item.name));
      copied++;
    }
  }
}
console.log(`Copied ${copied} files from official ABP package mappings in ${Date.now() - started}ms.`);
