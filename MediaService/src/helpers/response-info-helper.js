/**
 * Create an error response with the given status code, error and message
 * @author TaiPV
 * @createdDate 2024/12/21
 *
 * @param {number} statusCode
 * @param {string} error
 * @param {string} message
 * @returns
 */
const createEarlyErrorResponse = (statusCode, error, message) => {
    return {
        statusCode,
        error,
        message
    };
};

/**
 * Create a response info with the given data and resource name
 * @author TaiPV
 * @createdDate 2024/12/21
 *
 * @param {string} resourceName
 * @param {any} data
 */
const createResponseInfo = (resourceName, data) => {
    return {
        statusCode: 200,
        data: {
            [resourceName]: data
        }
    };
};

/**
 * Handle response info by returning the data with the given resource name
 * @author TaiPV
 * @createdDate 2024/12/21
 *
 * @param {string} resourceName
 * @param {any} data
 */
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
