const express = require("express");
const router = express.Router();
const { upload } = require("../middlewares/uploadFile");
const { verifyTokenAndAttachUser } = require("../middlewares/auth");
const MediaController = require("../controllers/media.controller");

/**
 * @swagger
 * tags:
 *   name: Medias
 *   description: Media management operations
 */

/**
 * @swagger
 * /media-service/api/:
 *   get:
 *     summary: |
 *       Get user's media storage info
 *       Created At: 2024/11/20
 *       Created by: ManhTD
 *     description: Get user's media storage info
 *     tags:
 *       - Medias
 *     x-author: John Doe
 *     x-created-at: "2024-11-20T12:00:00Z"
 *     responses:
 *       200:
 *         description: Successfully uploaded
 *       400:
 *         description: Error in upload
 *       500:
 *         description: Server error
 *     security:
 *       - BearerAuth: []
 */
router.get("/", verifyTokenAndAttachUser, MediaController.getMedia);

/**
 * @swagger
 * /media-service/api/upload-file:
 *   post:
 *     summary: |
 *       Upload a file
 *       Created At: 2024-11-20
 *       Created by: ManhTD
 *     description: Uploads a new file for a user.
 *     tags:
 *       - Medias
 *     requestBody:
 *       content:
 *         multipart/form-data:
 *           schema:
 *             type: object
 *             properties:
 *               file:
 *                 type: string
 *                 format: binary
 *                 description: The file to upload.
 *     responses:
 *       200:
 *         description: Successfully uploaded
 *       400:
 *         description: Error in upload
 *       500:
 *         description: Server error
 *     security:
 *       - BearerAuth: []
 */

router.post(
    "/upload-file",
    verifyTokenAndAttachUser,
    upload.single("file"),
    MediaController.uploadFile
);

module.exports = router;
