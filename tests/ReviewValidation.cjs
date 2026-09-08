// Pure JavaScript checks for the exported-document validator; no browser automation.
const fs = require('node:fs');
const vm = require('node:vm');
const assert = require('node:assert/strict');
const path = require('node:path');
const html = fs.readFileSync(path.join(__dirname, '../src/Nse.Pipeline/ReviewPage.html'), 'utf8');
const script = html.split('<script>')[1].split('</script>')[0];
new vm.Script(script); // Syntax-check the complete page script.
const validator = script.slice(script.indexOf('function validate(candidate)'), script.indexOf("$('reviewer').value=doc.reviewer.name;"));
const categories = ['objects', 'clothing', 'setting', 'actions'];
const original = {
  schemaVersion: '0.1', codebookVersion: 'visual-0.1', sourceSha256: 'a'.repeat(64), profileSha256: 'b'.repeat(64),
  reviewer: {name: 'test-reviewer', kind: 'ai_assisted'},
  frames: [{frameImage:'frames/frame-0000.jpg',timestampSeconds:2.5,categories:categories.map(category=>({category,status:'not_reviewed',observations:[]}))}]
};
const context = vm.createContext({original, names:Object.fromEntries(categories.map(c=>[c,c])), statuses:{not_reviewed:'',none_visible:'',observed:'',uncertain:''}});
vm.runInContext(validator, context);
function check(change, valid) {
  const candidate = structuredClone(original); change(candidate); context.candidate = candidate;
  if(valid) assert.doesNotThrow(()=>vm.runInContext('validate(candidate)',context));
  else assert.throws(()=>vm.runInContext('validate(candidate)',context));
}
check(()=>{},true);
check(d=>{d.reviewer.name='';},false);
check(d=>{d.sourceSha256='c'.repeat(64);},false);
check(d=>{d.frames[0].timestampSeconds=0;},false);
check(d=>{d.frames[0].categories[0].status='observed';},false);
check(d=>{d.frames[0].categories[0].category='clothing';},false);
check(d=>{d.frames[0].categories[0]={category:'objects',status:'uncertain',observations:[{label:'item',evidence:'Partly occluded rectangular item',certainty:'uncertain'}]};},true);
check(d=>{d.frames[0].categories[0]={category:'objects',status:'observed',observations:[{label:'item',evidence:'Partly occluded rectangular item',certainty:'uncertain'}]};},false);
console.log('PASS: page script syntax and eight export-validator cases. Browser rendering was not tested.');
