const express = require("express");
const router = express.Router();
const { verifyTokenAndAttachUser } = require("../middlewares/auth");

const templateController = require("../controllers/template.controller");

/**
 * @swagger
 * tags:
 *   name: Templates
 *   description: Template management operations
 */

/**
 * @swagger
 * /media-service/api/templates:
 *   get:
 *     summary: |
 *       Get all templates that can be used to create achievements
 *       Created At: 2024/12/09
 *       Created by: TaiPV
 *     tags:
 *       - Templates
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
router.get("/", verifyTokenAndAttachUser, templateController.getAllTemplates);

module.exports = router;
