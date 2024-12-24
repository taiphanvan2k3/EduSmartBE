const express = require("express");
const router = express.Router();
const { upload } = require("../middlewares/uploadFile");
const { verifyTokenAndAttachUser } = require("../middlewares/auth");

const achievementController = require("../controllers/achievement.controller");

/**
 * @swagger
 * tags:
 *   name: Achievements
 *   description: Achievement management operations
 */

/**
 * @swagger
 * /media-service/api/achievements/my-achievements/in-course/{courseId}:
 *   get:
 *     summary: |
 *       Retrieve the achievement export information and details for a specific course.
 *       Created At: 2024/12/21
 *       Created by: TaiPV
 *     tags:
 *       - Achievements
 *     parameters:
 *       - name: courseId
 *         in: path
 *         required: true
 *         schema:
 *           type: string
 *           format: uuid
 *         description: Id of course.
 *     responses:
 *       200:
 *         description: Achievement export information and details.
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 exportStatus:
 *                   type: object
 *                   properties:
 *                     canExport:
 *                       type: boolean
 *                       example: true
 *                       description: Indicates if the achievement can be exported.
 *                     exportedAchievement:
 *                       type: object
 *                       nullable: true
 *                       description: Details of the exported achievement, if available.
 *                       properties:
 *                         achievementURL:
 *                           type: string
 *                           example: "https://example.com/achievement"
 *                           description: URL to the exported achievement.
 *                         createdAt:
 *                           type: string
 *                           format: date-time
 *                           example: "2024-12-25T06:35:04.456Z"
 *                           description: Timestamp when the achievement was created.
 *                     achievementExportInfo:
 *                       type: object
 *                       description: Detailed configuration for the exportable achievement.
 *                       properties:
 *                         templateId:
 *                           type: integer
 *                           example: 1
 *                           description: Template ID used for exporting the achievement.
 *                         templateURL:
 *                           type: string
 *                         studentNameTextStyle:
 *                           type: object
 *                           description: Text style for the student's name.
 *                           properties:
 *                             color:
 *                               type: string
 *                               example: "#86358e"
 *                             fontSize:
 *                               type: string
 *                               example: "60px"
 *                             fontFamily:
 *                               type: string
 *                               example: "Dancing Script"
 *                         courseNameTextStyle:
 *                           type: object
 *                           description: Text style for the course name.
 *                           properties:
 *                             color:
 *                               type: string
 *                               example: "#333"
 *                             fontSize:
 *                               type: string
 *                               example: "48px"
 *                             fontFamily:
 *                               type: string
 *                               example: "Petrona"
 *                         dateTextStyle:
 *                           type: object
 *                           description: Text style for the date.
 *                           properties:
 *                             color:
 *                               type: string
 *                               example: "#333"
 *                             fontSize:
 *                               type: string
 *                               example: "24px"
 *                             fontFamily:
 *                               type: string
 *                               example: "Petrona"
 *                         teacherNameTextStyle:
 *                           type: object
 *                           description: Text style for the teacher's name.
 *                           properties:
 *                             color:
 *                               type: string
 *                               example: "#333"
 *                             fontSize:
 *                               type: string
 *                               example: "24px"
 *                             fontFamily:
 *                               type: string
 *                               example: "Alex Brush"
 *                         courseName:
 *                           type: string
 *                           description: Name of the course.
 *                         studentName:
 *                           type: string
 *                           description: Name of the student.
 *                         teacherName:
 *                           type: string
 *                           description: Name of the teacher.
 *       401:
 *         description: Unauthorized access. Missing or invalid token.
 *       404:
 *         description: Achievement not found for the given courseId.
 *     security:
 *       - BearerAuth: []
 */
router.get(
    "/my-achievements/in-course/:courseId",
    verifyTokenAndAttachUser,
    achievementController.getAchievementInCourse
);

/**
 * @swagger
 * /media-service/api/achievements:
 *   post:
 *     summary: |
 *       Generate a new achievement for a student
 *       Created At: 2024/12/21
 *       Created by: TaiPV
 *     tags:
 *       - Achievements
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               courseId:
 *                type: string
 *                format: uuid
 *     responses:
 *       200:
 *         description: Response info
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 id:
 *                   type: string
 *                   format: uuid
 *     security:
 *       - BearerAuth: []
 */
router.post(
    "/",
    verifyTokenAndAttachUser,
    achievementController.generateAchievement
);

/**
 * @swagger
 * /media-service/api/achievements/exports:
 *   post:
 *     summary: |
 *       Save the exported student achievement after the export operation is completed at Web
 *       Created At: 2024/12/24
 *       Created by: TaiPV
 *     description: Endpoint to export student achievements and save the data in the database.
 *     tags:
 *       - Achievements
 *     requestBody:
 *       required: true
 *       content:
 *         multipart/form-data:
 *           schema:
 *             type: object
 *             properties:
 *               courseId:
 *                 type: string
 *                 description: Id of course
 *               achievementFile:
 *                 type: string
 *                 format: binary
 *                 description: The file containing achievement data to be exported
 *             required:
 *               - courseId
 *               - achievementFile
 *     responses:
 *       200:
 *         description: Export and save operation completed successfully
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 success:
 *                   type: boolean
 *                   example: true
 *                 message:
 *                   type: string
 *                   example: "Export completed successfully."
 *       400:
 *         description: Bad request, invalid input
 *       401:
 *         description: Unauthorized
 *       500:
 *         description: Internal server error
 *     security:
 *       - BearerAuth: []
 */
router.post(
    "/exports",
    verifyTokenAndAttachUser,
    upload.single("achievementFile"),
    achievementController.saveExportedStudentAchievement
);

/**
 * @swagger
 * components:
 *   schemas:
 *     AchievementTemplate:
 *       type: object
 *       properties:
 *         id:
 *           type: integer
 *           example: 1
 *           description: Unique identifier for the achievement template
 *         name:
 *           type: string
 *           example: "Template_01"
 *           description: Name of the template
 *         thumbnailURL:
 *           type: string
 *           format: uri
 *           example: "https://res.cloudinary.com/da1aqhx1g/image/upload/c_thumb,w_200,g_face/v1733759920/1_pvlh7h.png"
 *           description: URL of the thumbnail for the template
 *         templateURL:
 *           type: string
 *           format: uri
 *           example: "https://res.cloudinary.com/da1aqhx1g/image/upload/v1733759920/1_pvlh7h.png"
 *           description: URL of the full template image
 *
 */

module.exports = router;
