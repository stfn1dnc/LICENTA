const fs = require('fs');
const path = require('path');

const envPath = path.resolve(__dirname, '.env');

console.log('Verific dacă există .env în:', envPath);
console.log('Există:', fs.existsSync(envPath));
