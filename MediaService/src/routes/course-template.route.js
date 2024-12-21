const express = require("express");
const router = express.Router();
const {
    verifyTokenAndAttachUser,
    isAllowForRoles
} = require("../middlewares/auth");

const templateController = require("../controllers/achievement-template.controller");

/**
 * @swagger
 * /media-service/api/course-templates/{id}:
 *   get:
 *     summary: |
 *       Get the course template information by ID
 *       Created At: 2024/12/21
 *       Created by: TaiPV
 *     tags:
 *       - Achievements
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         description: The unique identifier for the course template.
 *         schema:
 *           type: string
 *           format: uuid
 *     responses:
 *       200:
 *         description: A list of course templates associated with the provided courseId.
 *         content:
 *           application/json:
 *             schema:
 *                $ref: '#/components/schemas/CourseTemplate'
 *       404:
 *         description: No course template found for the given ID.
 *     security:
 *       - BearerAuth: []
 */
router.get(
    "/:id",
    verifyTokenAndAttachUser,
    isAllowForRoles(["Teacher"]),
    templateController.getCourseTemplateById
);

/**
 * @swagger
 * /media-service/api/course-templates/my-templates:
 *   get:
 *     summary: |
 *       Get all the created course templates in a the course
 *       Created At: 2024/12/16
 *       Created by: TaiPV
 *     tags:
 *       - Achievements
 *     parameters:
 *       - in: query
 *         name: courseId
 *         required: true
 *         description: The unique identifier for the course.
 *         schema:
 *           type: string
 *           format: uuid
 *     responses:
 *       200:
 *         description: A list of course templates associated with the provided courseId.
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 $ref: '#/components/schemas/CourseTemplate'
 *       400:
 *         description: Invalid request query
 *       404:
 *         description: No templates found for the given courseId.
 *     security:
 *       - BearerAuth: []
 */
router.get(
    "/my-templates",
    verifyTokenAndAttachUser,
    isAllowForRoles(["Teacher"]),
    templateController.getCourseTemplates
);

/**
 * @swagger
 * /media-service/api/course-templates:
 *   post:
 *     summary: |
 *       Create a new course template for the course
 *       Created At: 2024/12/13
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
 *               isDefault:
 *                 type: boolean
 *                 example: true
 *               courseId:
 *                type: string
 *                format: uuid
 *               templateId:
 *                 type: integer
 *                 description: ID of the achievement template
 *                 example: 1
 *               studentStyle:
 *                 type: object
 *                 properties:
 *                   fontFamily:
 *                     type: string
 *                     example: Arial
 *                   fontSize:
 *                     type: string
 *                     example: "12px"
 *                   color:
 *                     type: string
 *                     example: "#000000"
 *               courseNameStyle:
 *                 type: object
 *                 properties:
 *                   fontFamily:
 *                     type: string
 *                     example: Times New Roman
 *                   fontSize:
 *                     type: string
 *                     example: "14px"
 *                   color:
 *                     type: string
 *                     example: "#333333"
 *               dateStyle:
 *                 type: object
 *                 properties:
 *                   fontFamily:
 *                     type: string
 *                     example: Verdana
 *                   fontSize:
 *                     type: string
 *                     example: "10px"
 *                   color:
 *                     type: string
 *                     example: "#666666"
 *               teacherNameStyle:
 *                 type: object
 *                 properties:
 *                   fontFamily:
 *                     type: string
 *                     example: Helvetica
 *                   fontSize:
 *                     type: string
 *                     example: "12px"
 *                   color:
 *                     type: string
 *                     example: "#111111"
 *             required:
 *               - templateId
 *               - studentStyle
 *               - courseNameStyle
 *               - dateStyle
 *               - teacherNameStyle
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
    isAllowForRoles(["Teacher"]),
    templateController.createCourseTemplate
);

/**
 * @swagger
 * /media-service/api/course-templates/{id}:
 *   put:
 *     summary: |
 *       Update the course template for the course
 *       Created At: 2024/12/21
 *       Created by: TaiPV
 *     tags:
 *       - Achievements
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         description: The unique identifier for the course template.
 *         schema:
 *           type: string
 *           format: uuid
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               isDefault:
 *                 type: boolean
 *                 example: true
 *               studentStyle:
 *                 type: object
 *                 properties:
 *                   fontFamily:
 *                     type: string
 *                     example: Arial
 *                   fontSize:
 *                     type: string
 *                     example: "12px"
 *                   color:
 *                     type: string
 *                     example: "#000000"
 *               courseNameStyle:
 *                 type: object
 *                 properties:
 *                   fontFamily:
 *                     type: string
 *                     example: Times New Roman
 *                   fontSize:
 *                     type: string
 *                     example: "14px"
 *                   color:
 *                     type: string
 *                     example: "#333333"
 *               dateStyle:
 *                 type: object
 *                 properties:
 *                   fontFamily:
 *                     type: string
 *                     example: Verdana
 *                   fontSize:
 *                     type: string
 *                     example: "10px"
 *                   color:
 *                     type: string
 *                     example: "#666666"
 *               teacherNameStyle:
 *                 type: object
 *                 properties:
 *                   fontFamily:
 *                     type: string
 *                     example: Helvetica
 *                   fontSize:
 *                     type: string
 *                     example: "12px"
 *                   color:
 *                     type: string
 *                     example: "#111111"
 *             required:
 *               - templateId
 *               - studentStyle
 *               - courseNameStyle
 *               - dateStyle
 *               - teacherNameStyle
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
router.put(
    "/:id",
    verifyTokenAndAttachUser,
    isAllowForRoles(["Teacher"]),
    templateController.updateCourseTemplate
);

/**
 * @swagger
 * /media-service/api/course-templates/{id}:
 *   delete:
 *     summary: |
 *       Update the course template for the course
 *       Created At: 2024/12/21
 *       Created by: TaiPV
 *     tags:
 *       - Achievements
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         description: The unique identifier for the course template.
 *         schema:
 *           type: string
 *           format: uuid
 *     responses:
 *       200:
 *         description: Response info
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               properties:
 *                id:
 *                 type: string
 *                 format: uuid
 *     security:
 *       - BearerAuth: []
 */
router.delete(
    "/:id",
    verifyTokenAndAttachUser,
    isAllowForRoles(["Teacher"]),
    templateController.deleteCourseTemplate
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

/**
 * @swagger
 * components:
 *   schemas:
 *     CourseTemplate:
 *       type: object
 *       properties:
 *         id:
 *           type: string
 *           format: uuid
 *           description: The unique identifier for the template.
 *         achievementTemplateId:
 *           type: integer
 *           description: ID of the achievement template.
 *         studentStyle:
 *           type: object
 *           properties:
 *             fontFamily:
 *               type: string
 *               example: Arial
 *             fontSize:
 *               type: string
 *               example: "12px"
 *             color:
 *               type: string
 *               example: "#000000"
 *         courseNameStyle:
 *           type: object
 *           properties:
 *             fontFamily:
 *               type: string
 *               example: Times New Roman
 *             fontSize:
 *               type: string
 *               example: "14px"
 *             color:
 *               type: string
 *               example: "#333333"
 *         dateStyle:
 *           type: object
 *           properties:
 *             fontFamily:
 *               type: string
 *               example: Verdana
 *             fontSize:
 *               type: string
 *               example: "10px"
 *             color:
 *               type: string
 *               example: "#666666"
 *         teacherNameStyle:
 *           type: object
 *           properties:
 *             fontFamily:
 *               type: string
 *               example: Helvetica
 *             fontSize:
 *               type: string
 *               example: "12px"
 *             color:
 *               type: string
 *               example: "#111111"
 */

module.exports = router;
