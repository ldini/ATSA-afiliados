CREATE TABLE `cupones` (
  `CuponId` int(11) NOT NULL AUTO_INCREMENT,
  `CuponAfiliadoNombre` varchar(45) DEFAULT NULL,
  `CuponAfiliadoApellido` varchar(45) DEFAULT NULL,
  `CuponAfiliadoNro` varchar(45) DEFAULT NULL,
  `CuponOrdenNro` varchar(45) DEFAULT NULL,
  `CuponFechaGeneracion` varchar(45) DEFAULT NULL,
  `CuponCodigoPrestacion` varchar(45) DEFAULT NULL,
  `CuponCodigoPrestador` varchar(45) DEFAULT NULL,
  `CuponCantidad` int(11) DEFAULT NULL,
  PRIMARY KEY (`CuponId`)
) ENGINE=InnoDB AUTO_INCREMENT=74 DEFAULT CHARSET=utf8;
