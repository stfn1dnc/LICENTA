const express = require('express');
const router = express.Router();
const Log = require('../models/Log');
const authenticateToken = require('../middleware/authMiddleware.js');
const AppError = require('../utils/AppError');
const errors = require('../utils/errors');
// GET /api/logs
router.get('/', authenticateToken, async (req, res) => {
  try {
    const logs = await Log.find().populate('equipmentId userId');
    res.json(logs);
  } catch (err) {
  next(new AppError(errors.general.SERVER_ERROR, 500));;
  }
});

// POST /api/logs
router.post('/', authenticateToken, async (req, res) => {
  try {
    const { equipmentId, action, details } = req.body;

    const log = new Log({
      equipmentId,
      userId: req.user.id,
      action,
      details
    });

    const savedLog = await log.save();
    res.status(201).json(savedLog);
  } catch (err) {
     next(new AppError(errors.general,SERVER_ERROR, 500));
  }
});

module.exports = router;
