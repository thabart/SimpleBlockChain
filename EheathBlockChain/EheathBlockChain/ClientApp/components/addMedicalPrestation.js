import React from "react";

class Home extends React.Component {
    render() {
        return (<div>
            <h1>Ajouter nouvelle prestation médicale</h1>
            <form>
                <div className="row">
                    <div className="col-md-6">
                        <div className="form-group">
                            <label>Rechercher titulaire</label>
                            <input className="form-control" type="text" /> 
                            <button>Chercher</button>
                        </div>
                        <div className="form-group">
                            <label>Adresse du titulaire</label>
                        </div>
                        <div className="form-group">
                            <label>Nom et prénom du titulaire</label>
                        </div>
                        <div className="form-group">
                            <label>Organisme assureur</label>
                        </div>
                        <div className="form-group">
                            <label>Numéro d'inscriptions</label>
                        </div>
                    </div>
                   <div className="col-md-6">
                        <div className="form-group">
                            <label>Nom et prénom du patient</label>
                        </div>
                        <div className="form-group">
                            <label>Titulaire - conjoint - enfant - ascendant</label>
                        </div>
                        <div className="form-group">
                            <label>Date the naissance du patient</label>
                        </div>
                   </div>
                </div>
                <div className="row">
                    <div className="col-md-6">
                        <div className="form-group">
                            <label>Prestations et / ou fournitures</label>
                            <input className="form-control" type="text" />
                        </div>
                    </div>
                    <div className="col-md-6">
                        <div className="form-group">
                            <label>Diagnostic</label>
                            <textarea className="form-control"></textarea>
                        </div>
                    </div>
                </div>
                <div className="row">
                    <div className="col-md-6">
                        <div className="form-group">
                            <label>Nom de l'établissent</label>
                            <input className="form-control" type="text" />
                        </div>
                        <div className="form-group">
                            <label>Numéro d'identification</label>
                            <input className="form-control" type="text" />
                        </div>
                        <div className="form-group">
                            <label>Service</label>
                            <input className="form-control" type="text" />
                        </div>
                    </div>
                    <div className="col-md-6">
                        <div className="form-group">
                            <label>Nom et prénom du prescripteur</label>
                        </div>
                        <div className="form-group">
                            <label>Adresse du prescripteur</label>
                        </div>
                        <div className="form-group">
                            <label>Numéro INAMI</label>
                        </div>
                    </div>
                </div>
            </form>
        </div>);
    }
}

export default Home;