const express = require("express");
const cors = require("cors");
const createError = require("http-errors");
const errorHandler = require("./src/helpers/errorHandler");
const headers = require("./src/helpers/headers");
const logger = require("morgan");
const dotenv = require("dotenv").config();
const compression = require("compression");

// Swagger
const { swaggerUI, swaggerSpec } = require("./src/configs/swagger");

// Routers
const routes = require("./src/routes");

const app = express();

//register the endpoints
app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

app.use(logger("dev"));
app.use(
    compression({
        level: 6,
        threshold: 100 * 1000, // > 100kb thì mới nén
        filter: (req, res) => {
            if (req.headers["x-no-compression"]) {
                return false;
            }
            return compression.filter(req, res);
        }
    })
);

// Đường dẫn đến các file static
app.use("/media-service", express.static("public/media-service"));

routes(app);

app.use(
    "/media-service/swagger",
    swaggerUI.serve,
    swaggerUI.setup(swaggerSpec, {
        customCssUrl: "/media-service/swagger/custom-swagger.css",
        customJs: "/media-service/swagger/custom-swagger.js"
    })
);

app.use((req, res, next) => {
    next(createError(404, "Not found"));
});

app.use(headers);
app.use(errorHandler);

const PORT = process.env.PORT || 7154;

app.listen(PORT, "0.0.0.0", () => {
    console.log(`Server started on port ${PORT}...`);
});
