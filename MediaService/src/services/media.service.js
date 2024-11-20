const pool = require("../configs/database");

async function getUserStorageInfo(userId) {
    try {
        const query = 'SELECT * FROM "StorageInfos" where "UserId" = $1';
        const values = [userId];
        const { rows } = await pool.query(query, values);
        return rows;
    } catch (error) {
        console.error(error);
        throw new Error("Error fetching data from StorageInfo");
    }
}

async function updateStorageInfo(userId, extraStorage) {
    try {
        const querySelect =
            'SELECT "UsedStorage", "MaximumStorage" FROM "StorageInfos" WHERE "UserId" = $1';
        const result = await pool.query(querySelect, [userId]);

        if (result.rows.length > 0) {
            const userStorageInfo = result.rows[0];

            const extraStorageBytes = Math.round(extraStorage);
            const currentUsedStorage = Number(userStorageInfo.UsedStorage);
            const newUserStorage = currentUsedStorage + extraStorageBytes;

            const queryUpdate =
                'UPDATE "StorageInfos" SET "UsedStorage" = $1 WHERE "UserId" = $2';
            await pool.query(queryUpdate, [newUserStorage, userId]);
            return { message: "User storage updated successfully." };
        } else {
            throw new Error("User not found");
        }
    } catch (error) {
        console.error(error);
        throw new Error("Error updating StorageInfo: " + error.message);
    }
}

async function isEnoughStorage(userId, extraStorage) {
    try {
        const querySelect =
            'SELECT "UsedStorage", "MaximumStorage" FROM "StorageInfos" WHERE "UserId" = $1';
        const result = await pool.query(querySelect, [userId]);
        if (result.rows.length > 0) {
            const userStorageInfo = result.rows[0];

            const currentUsedStorage = Number(userStorageInfo.UsedStorage);
            const maximumStorage = Number(userStorageInfo.MaximumStorage);
            const newUserStorage = currentUsedStorage + extraStorage;

            return newUserStorage <= maximumStorage;
        } else {
            throw new Error("User not found");
        }
    } catch (error) {
        console.error(error);
        throw new Error("Error checking storage");
    }
}

module.exports = {
    getUserStorageInfo,
    updateStorageInfo,
    isEnoughStorage
};
