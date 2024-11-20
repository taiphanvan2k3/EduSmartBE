const jwt = require("jsonwebtoken");

const jwtSecret = process.env.JWT_SECRET;

function verifyTokenAndAttachUser(req, res, next) {
    const authHeader = req.headers["authorization"];
    const token = authHeader && authHeader.split(" ")[1]; // Extract the token from "Bearer <token>"

    if (!token) {
        return res.status(401).json({
            statusCode: 401,
            error: "Unauthorized",
            message: "You need to be authenticated to access this resource."
        });
    }

    try {
        const decoded = jwt.verify(token, jwtSecret);

        // Attach decoded token data to the request object
        req.user = {
            username: decoded.username,
            email: decoded.email,
            userId: decoded.userId,
            iss: decoded.iss,
            aud: decoded.aud,
            role: decoded.role,
            nbf: decoded.nbf,
            exp: decoded.exp,
            iat: decoded.iat
        };

        next(); // Proceed to the next middleware or route handler
    } catch (error) {
        return res.status(403).json({
            statusCode: 403,
            error: "Forbidden",
            message: "Invalid token"
        });
    }
}

module.exports = {
    verifyTokenAndAttachUser
};
