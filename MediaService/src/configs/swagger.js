const swaggerJSDoc = require("swagger-jsdoc");
const swaggerUI = require("swagger-ui-express");
const path = require("path");
const dotenv = require("dotenv").config();

const options = {
    definition: {
        openapi: "3.0.0",
        info: {
            title: "Media Service API",
            version: "1.0.0",
            description: "Media Service API"
        },
        servers: [
            {
                url: "http://localhost:" + process.env.PORT
            },
            {
                url: "https://edusmart.info.vn"
            }
        ],
        components: {
            securitySchemes: {
                BearerAuth: {
                    type: "http",
                    scheme: "bearer",
                    bearerFormat: "JWT"
                }
            }
        },
        security: [
            {
                BearerAuth: []
            }
        ]
    },
    apis: [path.join(__dirname, "../routes/*.js")]
};

const swaggerSpec = swaggerJSDoc(options);

module.exports = {
    swaggerSpec,
    swaggerUI
};
