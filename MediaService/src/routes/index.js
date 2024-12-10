const MediaRouter = require("./media.route");
const AchievementRouter = require("./achievement.route");

module.exports = (app) => {
    app.use("/media-service/api/media", MediaRouter);
    app.use("/media-service/api/achievements", AchievementRouter);
};
