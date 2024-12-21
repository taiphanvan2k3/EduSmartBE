const createEarlyErrorResponse = (statusCode, error, message) => {
    return {
        statusCode,
        error,
        message
    };
};

const createResponseInfo = (resourceName, data) => {
    return {
        [resourceName]: data
    };
};

module.exports = {
    createResponseInfo,
    createEarlyErrorResponse
};
