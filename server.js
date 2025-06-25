const express = require('express');
const mongoose = require('mongoose');
const authRoutes = require('./routes/auth');
const equipmentRoutes = require('./routes/equipment');
require('dotenv').config();

const app = express();
const PORT = process.env.PORT;
const logRoutes = require('./routes/logs');
const aiRoutes = require('./routes/ai');
const errorHandler = require('./middleware/errorHandler');
const cors = require('cors');

app.use(express.json()); // trebuie neapărat!

// Rute
app.use('/api/auth', authRoutes);
app.use('/api/equipment', equipmentRoutes);

app.use('/api/logs', logRoutes);
app.use('/api/ai',aiRoutes);
app.use(errorHandler);
app.use(cors());

app.get('/', (req, res) => {
  res.json({
    message: "API  is online!",
    endpoints: {
      login: "/api/auth/login",
      register: "/api/auth/register",
      chatPrompt: "/api/ai/prompt"
    }
  });
})
// Conexiune la Mongo + pornire server
mongoose.connect(process.env.MONGO_CONNECT_KEY)
  .then(() => {
    app.listen(PORT, '0.0.0.0', () => console.log("Server started on 0.0.0.0:8000"));
    console.log('Connected to MongoDB');
  })
  .catch(err => console.error(err));
