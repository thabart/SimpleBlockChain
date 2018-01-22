import React, { Component } from "react";
import { NavLink } from "react-router-dom";
import { SessionService } from './services';
import AppDispatcher from './appDispatcher';
import Constants from './constants';

class Header extends Component {
    constructor(props) {
        super(props);
        this._appDispatcher = null;
        this.disconnect = this.disconnect.bind(this);
        this.state = {
            isLoggedIn : false
        };
    }
    /* Disconnect the user */
    disconnect(e) {
        e.preventDefault();
        SessionService.remove();
        this.setState({
            isLoggedIn: false
        });
        AppDispatcher.dispatch({
            actionName: Constants.events.USER_LOGGED_OUT
        });
    }
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
                            <NavLink to="/" className="nav-link">Accueil</NavLink>
                        </li>
                        {(this.state.isLoggedIn && (
                            <li className="nav-item">
                                <NavLink to="/medicalprestation/add" className="nav-link">Ajouter prestation médicale</NavLink>
                            </li>
                        ))}
                        {(!this.state.isLoggedIn ? (
                            <li className="nav-item">
                                <NavLink to="/authenticate" className="nav-link">Se connecter</NavLink>
                            </li>
                        ) : (
                                <li className="nav-item">
                                    <a href="#" className="nav-link" onClick={this.disconnect}>Se déconnecter</a>
                                </li>
                        ))}
                    </ul>
                </div>
            </nav>    
        );
    }
    componentDidMount() {
        var self = this;
        self.setState({
            isLoggedIn: !SessionService.isExpired()
        });
        self._appDispatcher = AppDispatcher.register(function (payload) {
            switch (payload.actionName) {
                case Constants.events.USER_LOGGED_IN:
                    self.setState({
                        isLoggedIn: true
                    });
                    break;
            }
        });
    }
    componentWillUnmount() {
        AppDispatcher.unregister(this._appDispatcher);
    }
}

export default Header;