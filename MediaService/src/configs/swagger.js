const swaggerJSDoc = require("swagger-jsdoc");
const swaggerUI = require("swagger-ui-express");
const path = require("path");

const serverUrl =
    process.env.NODE_ENV === "development"
        ? {
              url: `http://localhost:${process.env.PORT}`,
              description: "Local server"
          }
        : {
              url: "https://edusmart.info.vn/",
              description: "Production"
          };

const options = {
    definition: {
        openapi: "3.0.0",
        info: {
            title: "Media Service API",
            version: "1.0.0",
            description: "Media Service API"
        },
        servers: [serverUrl],
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
