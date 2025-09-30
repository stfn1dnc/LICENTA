const Equipment = require('../models/Equipments');
const { validationResult } = require('express-validator');
const AppError = require('../utils/AppError');
const errors = require('../utils/errors');

// Controller and filtration
const getAllEquipment = async (req, res, next) => {
    try {
        const { status, location, type } = req.query;

        const filtrer = {};
        if (status) filtrer.status = status;
        if (location) filtrer.location = location;
        if (type) filtrer.type = type;

        const equipments = await Equipment.find(filtrer);
        res.json(equipments);
    } catch (err) {
        next(new AppError(errors.equipment.SERVER_ERROR, 500));
    }
};

const getEquipmentById = async (req, res, next) => {
    try {
        const equipment = await Equipment.findById(req.params.id);
        if (!equipment) {
            return next(new AppError(errors.equipment.NOT_FOUND, 404));
        }
        res.json(equipment);
    } catch (err) {
        next(new AppError(errors.equipment.SERVER_ERROR, 500));
    }
};

const createEquipment = async (req, res, next) => {
    const validation = validationResult(req);
    if (!validation.isEmpty()) {
        return next(new AppError(errors.equipment.VALIDATION_FAILED, 400));
    }

    try {
        const newEquipment = new Equipment(req.body);
        const saved = await newEquipment.save();
        res.status(201).json(saved);
    } catch (err) {
        next(new AppError(errors.equipment.SERVER_ERROR, 500));
    }
};

const updateEquipment = async (req, res, next) => {
    const Log = require('../models/Log');
    const validation = validationResult(req);
    if (!validation.isEmpty()) {
        return next(new AppError(errors.equipment.VALIDATION_FAILED, 400));
    }
    try {
        const updatedEquipment = await Equipment.findByIdAndUpdate(
            req.params.id,
            req.body,
            { new: true, runValidators: true }
        );
        if (!updatedEquipment) {
            return next(new AppError(errors.equipment.NOT_FOUND, 404));
        }
        res.json(updatedEquipment);
        await Log.create({
            equipmentId: updatedEquipment._id,
            userId: req.user.id,
            action: 'UPDATE',
            details: 'Updated equipment info'
        });
    } catch (err) {
        next(new AppError(errors.equipment.SERVER_ERROR, 500));
    }
};

const deleteEquipment = async (req, res, next) => {
    try {
        const deleted = await Equipment.findByIdAndDelete(req.params.id);
        if (!deleted) {
            return next(new AppError(errors.equipment.NOT_FOUND, 404));
        }
        res.json(deleted);
    } catch (err) {
        next(new AppError(errors.equipment.SERVER_ERROR, 500));
    }
};

module.exports = {
    getAllEquipment,
    getEquipmentById,
    createEquipment,
    updateEquipment,
    deleteEquipment
};
