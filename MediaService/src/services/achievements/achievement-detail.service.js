const pool = require("../../configs/database");
const { v4: uuidv4 } = require("uuid");
const { logInfo, logError } = require("../logger.service");
const {
    createEarlyErrorResponse,
    createResponseInfo
} = require("../../helpers/response-info-helper");
const { checkStudentCompletedCourse } = require("../grpc/grpc-course.service");
const { createAchievement } = require("../canvas.service");
const {
    uploadCloudinaryFromFilePath,
    deleteCloudinary
} = require("../../helpers/init-cloudinary");
const {
    getDefaultCourseTemplateInCourse
} = require("../course-templates/course-template-detail.service");
const { saveAchievementLocally } = require("../../helpers/utils");

/**
 * Generate achievement for student who completed the course
 * @author TaiPV
 * @createdDate 2024/12/21
 * @param {Guid} courseId
 * @param {number} studentId
 */
const generateAchievement = async (courseId, studentId) => {
    const caller = "generateAchievement";
    try {
        logInfo(caller, "Start");
        if (await checkIsExportedAchievement(courseId, studentId)) {
            return createEarlyErrorResponse(
                400,
                "Bad Request",
                "Achievement has already been exported"
            );
        }

        const grpcResponse = await checkStudentCompletedCourse(
            courseId,
            studentId
        );

        if (!grpcResponse.isSuccess) {
            return createEarlyErrorResponse(
                500,
                "Internal Server Error",
                grpcResponse.message
            );
        }

        if (!grpcResponse.isCompleted) {
            return createEarlyErrorResponse(
                400,
                "Bad Request",
                "Student has not completed the course"
            );
        }

        const defaultTemplateResponse =
            await getDefaultCourseTemplateInCourse(courseId);
        const defaultTemplate = defaultTemplateResponse.data.courseTemplate;
        if (!defaultTemplate) {
            return createEarlyErrorResponse(
                404,
                "Not Found",
                "Your teacher has not created any template for this course"
            );
        }

        const { localPath, localPathInPublic } = await createAchievement(
            defaultTemplate.templateId,
            grpcResponse.studentName,
            grpcResponse.courseName,
            grpcResponse.teacherName,
            defaultTemplate.studentNameTextStyle,
            defaultTemplate.courseNameTextStyle,
            defaultTemplate.dateTextStyle,
            defaultTemplate.teacherNameTextStyle
        );

        logInfo(caller, "End");
        return createResponseInfo("achievement", {
            courseId,
            localPath,
            localPathInPublic,
            createdAt: new Date().toISOString()
        });
    } catch (error) {
        logError(caller, error);
    }
};

/**
 * Save the achievement that student has exported to the database
 * @param {Guid} courseId
 * @param {number} studentId
 */
const saveExportedStudentAchievementFromWeb = async (
    courseId,
    studentId,
    studentName,
    achievementFile
) => {
    const caller = "saveExportedStudentAchievement";
    let publicId = null;

    try {
        logInfo(caller, "Start");

        if (await checkIsExportedAchievement(courseId, studentId)) {
            return createEarlyErrorResponse(
                400,
                "Bad Request",
                "Achievement has already been exported in this course"
            );
        }

        const isCompletedValidation = await validateCompletedCourse(
            courseId,
            studentId
        );

        if (isCompletedValidation.statusCode !== 200) {
            return isCompletedValidation;
        }

        const { localPath, localPathInPublic } = saveAchievementLocally(
            achievementFile,
            studentName
        );

        setImmediate(async () => {
            await saveExportedStudentAchievement(
                courseId,
                studentId,
                localPath
            );
        });

        return createResponseInfo("achievement", {
            courseId: courseId,
            localPath,
            localPathInPublic,
            createdAt: new Date().toISOString()
        });
    } catch (error) {
        if (publicId) {
            await deleteCloudinary(publicId);
        }

        logError(caller, error);
        throw error;
    } finally {
        logInfo(caller, "End");
    }
};

/**
 * Save the achievement that student has exported to the database
 * @param {Guid} courseId
 * @param {number} studentId
 * @param {string} localPath: Local path of the achievement file
 */
const saveExportedStudentAchievement = async (
    courseId,
    studentId,
    localPath
) => {
    const caller = "saveExportedStudentAchievement";
    try {
        logInfo(caller, "Start");

        const uploadResponse = await uploadCloudinaryFromFilePath(
            localPath,
            "achievements"
        );

        const query = `
            INSERT INTO "StudentAchievements" (
                "Id",
                "CourseId",
                "StudentId",
                "AchievementURL"
            )
            VALUES ($1, $2, $3, $4)
        `;

        const values = [
            uuidv4(),
            courseId,
            studentId,
            uploadResponse.secure_url
        ];

        await pool.query(query, values);
    } catch (error) {
        logError(caller, error);
    } finally {
        logInfo(caller, "End");
    }
};

/**
 * Get the exported achievement of a student in a course
 * @author TaiPV
 * @createdDate 2024/12/22
 * @param {Guid} courseId
 * @param {number} studentId
 */
const getExportedAchievement = async (courseId, studentId) => {
    const caller = "getExportedAchievement";
    try {
        logInfo(caller, "Start");

        const query = `
            SELECT "AchievementURL", "CreatedAt"
            FROM "StudentAchievements"
            WHERE "CourseId" = $1 AND "StudentId" = $2
        `;

        const { rows } = await pool.query(query, [courseId, studentId]);
        if (rows.length == 0) {
            return createEarlyErrorResponse(
                404,
                "Not Found",
                "Achievement not found"
            );
        }

        return createResponseInfo("achievement", {
            achievementURL: rows[0].AchievementURL,
            createdAt: rows[0].CreatedAt
        });
    } catch (error) {
        logError(caller, error);
    } finally {
        logInfo(caller, "End");
    }
};

const checkIsExportedAchievement = async (courseId, studentId) => {
    const caller = "checkIsExportedAchievement";
    try {
        logInfo(caller, "Start");
        const query = `
            SELECT 1 FROM "StudentAchievements"
            WHERE "CourseId" = $1 AND "StudentId" = $2
        `;

        const { rows } = await pool.query(query, [courseId, studentId]);
        return rows.length > 0;
    } catch (error) {
        logError(caller, error);
    }
};

const validateCompletedCourse = async (courseId, studentId) => {
    const grpcResponse = await checkStudentCompletedCourse(courseId, studentId);
    if (!grpcResponse.isSuccess) {
        return createEarlyErrorResponse(
            500,
            "Internal Server Error",
            grpcResponse.message
        );
    }

    if (!grpcResponse.isCompleted) {
        return createEarlyErrorResponse(
            400,
            "Bad Request",
            "Student has not completed the course"
        );
    }

    return createResponseInfo("statusCode", 200);
};

module.exports = {
    generateAchievement,
    saveExportedStudentAchievement,
    saveExportedStudentAchievementFromWeb,
    getExportedAchievement
};
