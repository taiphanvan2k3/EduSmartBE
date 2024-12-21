const createEarlyErrorResponse = (statusCode, error, message) => {
    return {
        statusCode,
        error,
        message
    };
};

const createResponseInfo = (resourceName, data) => {
    return {
        statusCode: 200,
        data: {
            [resourceName]: data
        }
    };
};

const handleResponseInfo = (resourceName, responseInfo) => {
    if (responseInfo.statusCode === 200) {
        return {
            [resourceName]: responseInfo.data[resourceName]
        };
    }

    return responseInfo;
};

module.exports = {
    createResponseInfo,
    createEarlyErrorResponse,
    handleResponseInfo
};
