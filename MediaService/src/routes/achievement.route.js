const express = require("express");
const router = express.Router();

const TemplateController = require("../controllers/achievement-template.controller");

/**
 * @swagger
 * tags:
 *   name: Achievements
 *   description: Achievement management operations
 */

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
router.get("/templates", TemplateController.getAchievementTemplates);

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
