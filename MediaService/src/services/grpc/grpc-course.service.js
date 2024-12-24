const grpc = require("@grpc/grpc-js");
const protoLoader = require("@grpc/proto-loader");
const fs = require("fs");
const path = require("path");

// Đường dẫn đến file .proto
const currentFolder = __dirname;
const PROTO_PATH = path.join(
    currentFolder,
    "..",
    "..",
    "protos",
    "course.proto"
);

// Tải file .proto
const packageDefinition = protoLoader.loadSync(PROTO_PATH, {
    keepCase: true,
    longs: String,
    enums: String,
    defaults: true,
    oneofs: true
});

let grpcEndpoint =
    process.env.NODE_ENV === "production"
        ? "courseservice:10002"
        : "localhost:10002";

const proto = grpc.loadPackageDefinition(packageDefinition);
const client = new proto.Course(
    grpcEndpoint,
    grpc.credentials.createInsecure()
);

const checkStudentCompletedCourse = (courseId, studentId) => {
    return new Promise((resolve, reject) => {
        client.CheckStudentCompletedCourse(
            { studentId, courseId },
            (err, response) => {
                if (err) {
                    reject(err);
                } else {
                    resolve(response);
                }
            }
        );
    });
};

module.exports = {
    checkStudentCompletedCourse
};
