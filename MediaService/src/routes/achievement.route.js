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
 *       Student get the achievement export status in a course
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
 *     responses:
 *       200:
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
 *                     achievement:
 *                       type: object
 *                       nullable: true
 *                       properties:
 *                         achievementURL:
 *                           type: string
 *                           example: "https://example.com/achievement"
 *                         createdAt:
 *                           type: string
 *                           format: date-time
 *                           example: "2024-12-25T06:35:04.456Z"
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
 *       Created At: 2024/12/21
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
