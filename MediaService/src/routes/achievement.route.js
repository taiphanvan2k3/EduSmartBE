const express = require("express");
const router = express.Router();
const { verifyTokenAndAttachUser } = require("../middlewares/auth");

const templateController = require("../controllers/achievement-template.controller");

/**
 * @swagger
 * tags:
 *   name: Achievements
 *   description: Achievement management operations
 */

/**
 * @swagger
 * /media-service/api/achievements/my-achievement-in-course/{courseId}:
 *   get:
 *     summary: |
 *       Get all achievements that teacher can choose to give to students
 *       Created At: 2024/12/09
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
 *         description: Achievement template URL with creation time
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                 statusCode:
 *                   type: integer
 *                   example: 200
 *                 data:
 *                   type: object
 *                   properties:
 *                     achievementURL:
 *                       type: object
 *                       properties:
 *                         AchievementURL:
 *                           type: string
 *                         CreatedAt:
 *                           type: string
 *                           format: date-time
 *     security:
 *       - BearerAuth: []
 */
router.get(
    "/my-achievement-in-course/:courseId",
    verifyTokenAndAttachUser,
    templateController.getAchievementInCourse
);

/**
 * @swagger
 * /media-service/api/achievements/templates:
 *   get:
 *     summary: |
 *       Get all achievements that teacher can choose to give to students
 *       Created At: 2024/12/09
 *       Created by: TaiPV
 *     tags:
 *       - Achievements
 *     responses:
 *       200:
 *         description: List of achievement templates
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/AchievementTemplate'
 *     security:
 *       - BearerAuth: []
 */
router.get(
    "/templates",
    verifyTokenAndAttachUser,
    templateController.getAchievementTemplates
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
    templateController.generateAchievement
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
