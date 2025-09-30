const express = require('express');
const router = express.Router();
const {askAI}= require('../controllers/aiController');
const authenticateToken = require('../middleware/authMiddleware.js');

router.post('/prompt', authenticateToken, askAI);

module.exports = router