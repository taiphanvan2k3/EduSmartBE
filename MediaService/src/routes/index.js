const MediaRouter = require("./media.route");
const AchievementRouter = require("./achievement.route");
const CourseTemplateRouter = require("./course-template.route");

module.exports = (app) => {
    app.use("/media-service/api/media", MediaRouter);
    app.use("/media-service/api/achievements", AchievementRouter);
    app.use("/media-service/api/course-templates", CourseTemplateRouter);
};
