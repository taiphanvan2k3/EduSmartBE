const fs = require('fs').promises;
const path = require('path');
const { format } = require('date-fns');

const logDirectory = path.join(__dirname, '../Logs');
const fileName = path.join(logDirectory, 'logs.log');
const logEvents = async (msg) => {
    const dateTime = `${format(new Date(), `dd-MM-yyyy\tHH:mm:ss`)}`;
    const contentLog = `${dateTime} ----- ${msg}\n`;

    try {
        // Check if the directory exists, if not, create it
        await fs.mkdir(logDirectory, { recursive: true });

        // Append the log content to the log file
        await fs.appendFile(fileName, contentLog);
    } catch (error) {
        console.error('Error writing log:', error);
    }
};
module.exports = logEvents;
