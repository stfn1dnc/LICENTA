const errors = {
    auth: {
        MISSING_TOKEN: 'Acces denied! No Token found.',
        INVALID_TOKEN: 'Invalid or expired Token!',
        USER_EXISTS: 'User already exists!',
        INVALID_CREDENTIALS: 'Invalid credentials!',
        REGISTER_SERVER: 'Server error during registration!',
        LOGIN_SERVER: 'Server error during login!',
    },
    equipment: {
        NOT_FOUND: 'Equipment not found!',
        SERVER_ERROR: 'SERVER ERROR!',
        VALIDATION_ERROR: 'Invalid credentials!'
    },
    general: {
        SERVER_ERROR: 'Internal server error!',
        BAD_REQUEST: 'Bad request!',
    }, 
    ai: {
        ERROR_FROM_OPENAI: 'Error from the AI suplier'
    }
};

module.exports = errors;