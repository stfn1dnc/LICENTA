const mongoose = require('mongoose');

// Definim schema = ce câmpuri va avea un echipament
const EquipmentSchema = new mongoose.Schema({
  name: String,
  location: String,
  status: String,
  last_maintenance: String,
  type: String
});

// Exportăm modelul ca să l folosim în rute
module.exports = mongoose.model('Equipment', EquipmentSchema);
