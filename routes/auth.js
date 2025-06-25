const express = require('express');
const router = express.Router();
const jwt = require('jsonwebtoken');
const User = require('../models/User');
const AppError = require('../utils/AppError');
const errors = require('../utils/errors');
require('dotenv').config();

// REGISTER
router.post('/register', async (req, res, next) => {
  try {
    const { username, email, password } = req.body;

    const existingUser = await User.findOne({ $or: [{ email }, { username }] });
    if (existingUser)
      return next(new AppError(errors.auth.USER_EXISTS, 400));

    const user = new User({ username, email, password });
    await user.save();

    res.status(201).json({ message: 'User registered successfully!' });
  } catch (err) {
    console.error(err);
    next(new AppError(errors.auth.REGISTER_SERVER, 500));
  }
});

// LOGIN
router.post('/login', async (req, res, next) => {
  try {
    const { email, password } = req.body;

    const user = await User.findOne({ email });
    if (!user || !(await user.comparePassword(password))) {
      return next(new AppError(errors.auth.INVALID_CREDENTIALS, 401));
    }

    const token = jwt.sign({ id: user._id }, process.env.JWT_SECRET, {
      expiresIn: '2h',
    });

    res.json({ token });
  } catch (err) {
    console.error(err);
    next(new AppError(errors.auth.LOGIN_SERVER, 500));
  }
});

module.exports = router;
