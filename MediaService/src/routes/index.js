const MediaRouter = require("./media.route");

module.exports = (app) => {
    app.use("/media-service/api", MediaRouter);
};
