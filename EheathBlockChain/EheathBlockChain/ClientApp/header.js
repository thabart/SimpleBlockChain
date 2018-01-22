import React, { Component } from "react";
import { NavLink } from "react-router-dom";

class Header extends Component {
    render() {
        return (
            <nav className="navbar navbar-toggleable-md navbar-light bg-faded">
                <button className="navbar-toggler navbar-toggler-right" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                    <span className="navbar-toggler-icon"></span>
                </button>
                <a className="navbar-brand" href="#">Navbar</a>
                <div className="collapse navbar-collapse" id="navbarSupportedContent">
                    <ul className="navbar-nav mr-auto">
                        <li className="nav-item active">
                            <NavLink to="/">Accueil</NavLink>
                        </li>
                        <li className="nav-item">
                            <NavLink to="/medicalprestation/add">Ajouter prestation médicale</NavLink>
                        </li>
                        <li className="nav-item">
                            <NavLink to="/authenticate">Se connecter</NavLink>
                        </li>
                    </ul>
                </div>
            </nav>    
        );
    }
}

export default Header;