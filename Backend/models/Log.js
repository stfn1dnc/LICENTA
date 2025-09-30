const mongoose = require ('mongoose');

const logSchema = new mongoose.Schema({
    equipmentId: {
        type: mongoose.Schema.Types.ObjectId,
        ref: "Equipment",
        required: true
    },
    userId: {
        type: mongoose.Schema.Types.ObjectId,
        ref: "User",
        required: true
    },
    action: {
        type: String,
        enum: ['CREATE','UPDATE','READ','DELETE'],
        required: true
    },
    timestamps: {
        type: Date,
        default: Date.now
    },
    details: {
type: String
    }
});

module.exports = mongoose.model('Log', logSchema);