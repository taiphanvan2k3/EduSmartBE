const createEarlyErrorResponse = (statusCode, error, message) => {
    return {
        statusCode,
        error,
        message
    };
};

module.exports = {
    createEarlyErrorResponse
};
