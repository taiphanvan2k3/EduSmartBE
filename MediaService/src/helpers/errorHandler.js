const logEvents = require('./logEvents');
const { v4: uuid } = require('uuid');

module.exports = (err, req, res, next) => {
    const statusCode = err.status || 500;
    const errorType = err.name || 'Error';
    const errorMessage = err.message || 'An unexpected error occurred';

    logEvents(`idError ----- ${uuid()} ----- ${req.url} ----- ${req.method} ----- ${err.message}`);
    
    res.status(statusCode).json({
        statusCode: statusCode,
        error: errorType,
        message: errorMessage,
    });
};
