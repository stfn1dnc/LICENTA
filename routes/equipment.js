const express = require('express');
const router = express.Router();
const { body, validationResult } = require('express-validator');
const authenticateToken = require('../middleware/authMiddleware.js');



const {
  getAllEquipment,
  getEquipmentById,
  createEquipment,
  updateEquipment,
  deleteEquipment
} = require('../controllers/equipmentController');

//protejeaza rutele sensibile
router.post('/', authenticateToken, createEquipment);
router.put('/:id', authenticateToken, updateEquipment);
router.delete('/:id', authenticateToken, deleteEquipment);

// GET all
router.get('/', authenticateToken, getAllEquipment);

// GET by ID
router.get('/:id', authenticateToken, getEquipmentById);

// POST (creare)
router.post(
  '/',
  authenticateToken,
  [
    body('name').notEmpty().withMessage('Name is required!'),
    body('location').notEmpty().withMessage('Location is required!'),
    body('status').isIn(['OK', 'NEEDS_MAINTENANCE']).withMessage('Status must be OK or NEEDS_MAINTENANCE!'),
    body('last_maintenance').notEmpty().withMessage('Last maintenance date is required!'),
    body('type').notEmpty().withMessage('Type is required!')
  ],
  createEquipment
);

// PUT (modificare)
router.put('/:id',
  authenticateToken,
  [
    body('name').notEmpty().withMessage('Name is required!'),
    body('location').notEmpty().withMessage('Location is required!'),
    body('status').isIn(['OK', 'NEEDS_MAINTENANCE']).withMessage('Status must be OK or NEEDS_MAINTENANCE!'),
    body('last_maintenance').notEmpty().withMessage('Last maintenance date is required!'),
    body('type').notEmpty().withMessage('Type is required!')
  ],
  updateEquipment
);

// DELETE
router.delete('/:id', authenticateToken, deleteEquipment);


module.exports = router;
