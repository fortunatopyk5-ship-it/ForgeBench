# YM-CROSSOVER-1 technical specification — Iteration 2

This file fixes one concrete candidate state space and landing criterion. It is a research specification, not a claim that Balaban's UV construction has already been proved to land in this space.

## 1. Coarse lattice and polymers

At RG scale k write a_k=b^k a and rescale coordinates so the coarse lattice is the unit hypercubic lattice in d=4.

A **block** B is a unit 4-cell. Two blocks are adjacent when they share a 3-face.

A **polymer** X is a finite face-connected set of blocks.

Notation:
- |X| = number of blocks;
- diam(X) = graph diameter in the block-adjacency metric;
- X^+ = one-block collar of X;
- E(X^+) = oriented lattice links with both endpoints in X^+.

The collar is included so that boundary-field dependence is explicit. A local interaction on X is allowed to depend on all link variables in E(X^+); the variables in E(X^+)\E(X) are its boundary fields.

## 2. Local gauge-invariant function space

Fix an Ad-invariant inner product on the Lie algebra g and an orthonormal basis {T^a}. For an oriented link e and unit xi in g define the left-invariant derivative

D_{e,xi} F(U)
 = d/dt F(..., exp(t xi) U_e, ...)|_{t=0}.

For integer p>=0 and rho>0 define

||F||_{p,rho;X}
 = max_{0<=m<=p} (rho^m/m!)
   sup_{U in G^{E(X^+)}}
   sup_{e_1,...,e_m; |xi_i|=1}
   |D_{e_1,xi_1}...D_{e_m,xi_m}F(U)|.

The m=0 term is ||F||_infinity. Thus boundary fields are controlled uniformly by the same supremum.

C^p_gi(X) is the real Banach space of C^p functions on G^{E(X^+)} invariant under lattice gauge transformations
U_e -> h_{s(e)} U_e h_{t(e)}^{-1}
for all vertices meeting E(X^+).

Normalization: each Phi_X is required to have zero full Haar mean on its local variables,

int Phi_X(U) product_{e in E(X^+)} dU_e = 0.

This removes additive constants polymer-by-polymer. It does not make the decomposition unique; a fixed localization/extraction prescription is therefore part of the definition of the RG map.

## 3. Candidate Banach space

For alpha,mu,rho>0 and p>=0 define

||Phi||_{alpha,mu,p,rho}
 = sup_B sum_{X contains B}
   exp(alpha |X| + mu diam(X)) ||Phi_X||_{p,rho;X}.

B_{alpha,mu,p,rho} is the completion of finite-range translation-covariant gauge-invariant interactions in this norm.

B^sym is the closed subspace invariant under lattice translations, hypercubic rotations/reflections and CP (theta=0 sector).

Because the norm is an anchored weighted l^1 norm of Banach-valued components, B is Banach.

Important choice: there is NO separate unbounded "large field" coordinate in this global group-valued space; G is compact. Balaban's small-field/large-field representation must therefore be converted into globally defined Phi_X. That conversion is an additional lemma (YM-MATCH-EXTRACT-1), not something inferred from UV stability automatically.

## 4. Strong-coupling coordinate and remainder

Let W be a fixed normalized one-block Wilson plaquette interaction in B^sym, with ||W|| finite.

Fix a bounded linear functional ell_W on B^sym with ell_W(W)=1. One concrete choice is the Haar-L2 projection of the one-block localized interaction onto the centered plaquette class function.

Define
beta(Phi)=ell_W(Phi),
R(Phi)=Phi-beta(Phi) W,
r(Phi)=||R(Phi)||_{alpha,mu,p,rho}.

This beta is a tube coordinate, not automatically identical to the perturbative running coupling used in Balaban's small-field chart. Relating the two is part of YM-MATCH-EXTRACT-1.

A quantitative matching set is

K_match(beta_-,beta_+,eps)
 = {Phi in B^sym:
      beta_- <= beta(Phi) <= beta_+,
      r(Phi) <= eps}.

For the crossover route beta is expected to decrease toward the product-Haar/strong-coupling side.

## 5. Exact RG map

Choose a gauge-covariant block kernel Q_b(V|U). A concrete globally defined candidate is a product of central heat kernels around gauge-covariant path products:

Q_b(V|U)
 = product_{E coarse} q_tau( V_E B_E(U)^{-1} ),

where B_E(U) is the ordered product of fine links along the chosen block path, and q_tau is a normalized central heat kernel on G.

Gauge covariance follows because B_E transforms at its endpoints and q_tau is conjugation invariant.

For a finite volume define the exact coarse density

exp[-S'(V)]
 = Z^{-1} int Q_b(V|U) exp[-S_Phi(U)] dU.

Then:
1. R_block chooses V and Q_b;
2. R_fluctuation is the exact conditional integration over U;
3. R_rescale identifies the b-spaced coarse lattice with unit spacing;
4. R_localize applies the fixed localization prescription to -log density and returns Phi'.

Thus the actual map is a composition

R = R_localize o R_rescale o R_fluctuation o R_block.

The pushforward density exists in finite volume. The nontrivial estimate is that R(Phi) again belongs to B with volume-independent norm bounds.

## 6. Polymerization and D_KP

Given a global interaction Phi, write

exp[-sum_X Phi_X]
 = product_X (1 + zeta_X),
zeta_X = exp(-Phi_X)-1.

After expanding the product, group each connected family of overlapping interaction supports into its union P. Integrating the fine link variables internal to each disconnected component against product Haar gives an exact hard-core polymer gas

Xi_Lambda = sum_{Gamma pairwise disjoint} product_{P in Gamma} z_Phi(P).

This identity is the definition of the scalar super-polymer activity z_Phi(P).

Define

Q_{alpha_KP,mu_KP}(Phi)
 = sup_B sum_{P contains B}
   |z_Phi(P)|
   exp(alpha_KP |P| + mu_KP diam(P)).

For fixed alpha_KP,mu_KP>0 and q<alpha_KP set

D_KP(alpha_KP,mu_KP,q)
 = {Phi: Q_{alpha_KP,mu_KP}(Phi) <= q}.

Why q<alpha_KP is sufficient:
for any polymer P0,

sum_{P intersect P0}
 |z(P)| exp(alpha_KP |P| + mu_KP diam(P))
 <= |P0| q
 < alpha_KP |P0|.

This is exactly the Kotecky-Preiss criterion with
a(P)=alpha_KP |P| and
g(P)=mu_KP diam(P).

Consequences:
- absolute cluster-expansion convergence uniformly in finite volume;
- exponential tail control for clusters;
- for bounded local coarse observables F,G,
  |Cov(F,G)| <= C_{F,G} exp[-mu' d(supp F,supp G)]
  for any fixed mu'<mu_KP after reserving a small part of the exponential weight for source attachments.

The prefactor depends on the fixed observable supports but not on total volume.

Gauge fixing is not used: all integrations are over compact link Haar measure and observables/interactions are gauge invariant.

## 7. Critical correction: measure landing is not enough

For an exact coarse variable V,

Cov_mu(F,G)
 = Cov_{mu'}( E[F|V], E[G|V] )
   + E_{mu'}[ Cov(F,G|V) ].

Therefore Phi' in D_KP controls only the first term unless the RG also controls source transport / conditional covariance.

A toy counterexample is a product of:
- visible variables V with product measure;
- hidden variables H with long-range correlations;
with an RG that discards H. The coarse measure is already in D_KP, while observables of H retain long-range correlations.

Hence the corrected landing package is:

MEASURE:
R^M(K_match) subset D_KP.

SOURCE:
source-dependent RG derivatives / conditional covariances generated in all eliminated shells obey a common exponentially local bound, so microscopic local gauge-invariant observables are represented by quasi-local coarse sources with summable tails.

This source condition is YM-RG-SOURCE-1.

## 8. Invariant tube form

Set
K_j = {Phi in B^sym:
       beta(Phi) in I_j=[beta_j^-,beta_j^+],
       r(Phi) <= R_j}.

A one-step certificate consists of finite inequalities valid for all Phi in K_j:

F_j^-(beta,r) <= beta(R Phi) <= F_j^+(beta,r),
r(R Phi) <= G_j(beta,r),

plus
inf_{K_j}F_j^- >= beta_{j+1}^-,
sup_{K_j}F_j^+ <= beta_{j+1}^+,
sup_{K_j}G_j <= R_{j+1}.

At the final step one also proves

sup_{Phi in K_M} Q_{alpha_KP,mu_KP}(Phi) <= q < alpha_KP.

Then induction gives
R(K_j) subset K_{j+1}
and
R^M(K_match) subset D_KP.

The exact-rational verifier in scripts/verify_crossover_tube.py checks this finite induction once rigorous one-step enclosure formulae are provided.

## 9. What is known from Balaban vs additional

KNOWN FROM BALABAN (at the level verified in the primary-paper metadata/abstracts and review literature):
- gauge-field averaging/RG operations were constructed and analyzed;
- in 4D, small-field effective actions and coupling renormalization were developed;
- the fluctuation integral was exponentiated by cluster expansion and shown to preserve the small-field inductive form;
- large-field R operations were constructed; the 1989 Part II states completion of the ultraviolet-stability proof for four-dimensional pure gauge theories.

NOT obtained merely from those statements:
- a cutoff/volume-uniform compact K_match in the global Banach norm above at a fixed g_match;
- a proof that beta(Phi) in this global chart is quantitatively equivalent to Balaban's perturbative coupling coordinate at matching;
- a finite-step nonperturbative invariant tube all the way to D_KP;
- source/observable transport bounds sufficient to transfer final coarse mixing to all microscopic local observables;
- the complete R^4 continuum OS theory.

These are explicit additional tasks, not consequences of the phrase "UV stability".
