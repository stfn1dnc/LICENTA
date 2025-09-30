const jwt = require('jsonwebtoken');
require('dotenv').config();
const AppError = require('../utils/AppError');
const errors = require('../utils/errors');

const authenticateToken = (req, res, next) => {
    const authHeader = req.headers['authorization'];
    const token = authHeader && authHeader.split(' ')[1];

    if (!token) {
        return next(new AppError(errors.auth.MISSING_TOKEN, 401));
    }

    try {
        const decoded = jwt.verify(token, process.env.JWT_SECRET);
        req.user = decoded;
        next();
    } catch (err) {
        return next(new AppError(errors.auth.INVALID_TOKEN, 403));
    }
};

module.exports = authenticateToken;
