CREATE DATABASE w1362013_conaf;
USE w1362013_conaf;

CREATE TABLE `usuario` (
  `UsuarioId` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Apellido` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Email` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Clave` char(32) NOT NULL,
  `FechaHoraAlta` datetime NOT NULL,
  `FechaHoraBaja` datetime DEFAULT NULL,
  `Token` char(32) DEFAULT NULL,
  `EsAdmin` bit(1) NOT NULL,
  PRIMARY KEY (`UsuarioId`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=latin1;

CREATE TABLE HistoricoConsultas (
	HistoricoConsultasId BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT,
    NumeroAfiliadoConsultado NVARCHAR(20),
    FechaHora DATETIME
);
