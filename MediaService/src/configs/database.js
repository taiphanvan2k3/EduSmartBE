const { Pool } = require("pg");
require("dotenv").config();

// Create a new pool using the connection string
const pool = new Pool({
    user: process.env.DB_USERNAME,
    host: process.env.DB_HOST,
    database: process.env.DB_DATABASE_PAYMENT,
    password: process.env.DB_PASSWORD,
    port: process.env.DB_PORT,
    ssl: process.env.DB_SSL === "true"
        ? { rejectUnauthorized: process.env.DB_TRUST_CERT !== "true" }
        : false
});

module.exports = pool;
