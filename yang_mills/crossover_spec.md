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

For the old crossover chart beta was expected to decrease toward the product-Haar/strong-coupling side. Iteration 3 does NOT use this as an identified Balaban coordinate. The source-faithful marginal coordinate is the displayed classical-action parameter c=1/g^2; comparison with beta=ell_W(Phi) remains an open normalization/dictionary lemma.

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
- a cutoff/volume-uniform closed/bounded K_match in the global Banach norm above at a fixed g_match;
- a proof that beta(Phi) in this global chart is quantitatively equivalent to Balaban's perturbative coupling coordinate at matching;
- a finite-step nonperturbative invariant tube all the way to D_KP;
- source/observable transport bounds sufficient to transfer final coarse mixing to all microscopic local observables;
- the complete R^4 continuum OS theory.

These are explicit additional tasks, not consequences of the phrase "UV stability".


## 10. Corrections after adversarial audit

### 10.1 Polymer incompatibility uses actual collars/supports
Because Phi_X is a function of links in X^+, two interaction polymers are incompatible whenever their actual link-supports overlap, not merely when their core block sets X overlap.

Define supp(Phi_X) as the set of blocks meeting E(X^+). All super-polymer unions, diameters used in the KP activity, and incompatibility relations are henceforth understood in terms of these actual support blocks. The anchored KP proof is unchanged after this replacement.

### 10.2 D_KP is sufficient, not necessary
The fixed product-Haar polymer chart can fail even for a perfectly mixing measure. Example: a product measure over blocks with a very large one-block potential has exactly zero connected correlations between distinct blocks, but the activity exp(-Phi_B)-1 can have arbitrarily large norm and violate Q_{alpha,mu}<alpha.

Therefore failure to enter this particular D_KP does not imply absence of a mass gap. YM-CROSSOVER-1 is a sufficient route and may be strictly stronger than the desired physical statement.

### 10.3 Source data must already be present at matching
Microscopic observables pass through the many UV steps before K_match is reached. Thus the UV-to-matching package cannot consist only of a measure interaction Phi. It must also provide a controlled source chart for the chosen local gauge-invariant observable algebra.

The corrected matching object is a pair
(Phi,J-map)
with Phi in K_match and source kernels whose quasi-local norms are uniformly bounded. YM-MATCH-EXTRACT-1 must be strengthened accordingly, or an explicit UV-SOURCE-MATCH lemma must be supplied.

### 10.4 Finite-drift terminal strip
A practical drift certificate should not assume that every beta<=beta_sc belongs to D_KP. Use a bounded terminal strip
beta_min <= beta <= beta_sc
and prove a no-overshoot lower bound for the last step. This is encoded in YM-RG-DRIFT-2.


## 11. Iteration 3 correction: compactness

The matching tube
K_match={Phi in B^sym: beta_-<=beta(Phi)<=beta_+, r(Phi)<=R_match}
is CLOSED and BOUNDED when beta is continuous and r(Phi)=||Q Phi|| with Q bounded. It is not claimed compact.

No existing tube-induction argument requires compactness. Expressions such as
sup_{Phi in K_j} F(Phi)
are supremums in the extended real numbers and need not be attained. Every use in a proof must supply a finite upper bound explicitly. Continuity on a closed bounded infinite-dimensional set does not by itself imply such a bound.

If future work requires attainment/subsequence compactness, a separate compact-embedding/tighter-topology lemma must be stated.

## 12. Iteration 3 verdict on the global all-field C^p matching norm

The space B_{alpha,mu,p,rho} defined above remains a mathematically legitimate Banach space, but it is NOT presently a legitimate Balaban matching space.

The reason is structural.

Balaban's full effective density is represented sector-by-sector, with admissible domain histories, characteristic factors, a T-operation, and an E/R/B effective action. The available analytic bounds for local E/R/B terms are stated on restricted regularity/analyticity domains. Large-field pieces are controlled by separate localized activity estimates and large-field suppression. These estimates do not give uniform derivatives of a single recombined interaction over every U in G^{E(X^+)}.

In particular, a bound that is small because a large-field sector carries an activity/probability factor does not imply
sup_U |D^m Phi_X(U)| << 1.
Rare configurations are not discounted by the old local norm.

A literal absorption of sharp characteristic factors into Phi_X is also incompatible with C^p regularity at sector boundaries. The exact total density may nevertheless be smooth after summing sectors; what is missing is a theorem controlling derivatives of that recombination. Therefore the rigorous conclusion is:

[NOT ESTABLISHED]
Balaban output => ||Q Phi_match||_{alpha,mu,p,rho} <= epsilon_match.

This is a mismatch of known estimates/topology, not a proof that no globally smooth representation exists.

## 13. Candidate Balaban-native matching topology

To avoid differentiating sector characteristic functions, keep the domain history as a combinatorial label.

Let Sigma be an admissible domain-history label (Omega_j,Lambda_j,S_j, etc.) and let d_Sigma(X) denote the native localization metric attached to the corresponding scale/sector. Let U^c_Sigma(X) be the restricted analytic/regularity domain on which the local activity is controlled.

For a local analytic activity F_{Sigma,X}, define

||F_{Sigma,X}||^{an}_{p,rho}
 =
 max_{0<=m<=p} rho^m/m!
 sup_{U in U^c_Sigma(X)}
 sup_{e_i,|xi_i|=1}
 |D_{e_1,xi_1}...D_{e_m,xi_m}F_{Sigma,X}(U)|.

For kappa'>0 define the anchored native activity norm

N_{kappa',p,rho}(F)
 =
 sup_{Sigma,B}
 sum_{X contains B}
 exp(kappa' d_Sigma(X))
 ||F_{Sigma,X}||^{an}_{p,rho}.

The characteristic/domain indicator is NOT differentiated; it belongs to Sigma.

A candidate matching datum is the tuple
D=(g,{E_Sigma},{R_Sigma},{B_Sigma},{C_Sigma},...)
with norm assembled componentwise, for example

||D||_native
 =
 |c(g)|
 + w_E N(E)
 + w_R N(R)
 + w_B N(B)
 + w_C N(C),

where the weights are chosen to normalize the native amplitudes appearing in the source bounds (such as powers of g or exp[-p_0(g)]). The exact weights are part of YM-MATCH-EXTRACT-1 and are not fixed here without source justification.

This is a candidate topology because its geometry matches the form of the extracted Balaban estimates. Preservation by the full RG still has to be proved/identified with Balaban's inductive class.

### Rooted-tail conversion needed

A pointwise source estimate
||F_{Sigma,X}|| <= A(g) exp(-kappa d_Sigma(X))
does not automatically imply N_{kappa'}(F)<infinity.

It suffices to prove a volume-uniform rooted counting bound
# {X contains B: n<=d_Sigma(X)<n+1} <= C_count exp(c_count n)
and choose kappa-kappa'>c_count. Then

N_{kappa'}(F)
 <= A(g) C_count
    sum_{n>=0} exp[-(kappa-kappa'-c_count)n],

which is finite and explicit.

This counting/tail conversion is a separate requirement, not hidden inside the notation.

## 14. Natural source extension

The native sector/activity topology has a direct source extension. If J denotes quasi-local source kernels indexed by the same (Sigma,X), define

||(D,J)||_src = ||D||_native + lambda_J N_{kappa'_J,p_J,rho_J}(J).

Thus the corrected topology can in principle accommodate YM-UV-SOURCE-MATCH-1 / YM-RG-SOURCE-1 without changing representation. No source theorem is proved by this observation.

## 15. Full RG is E then S then L, not just E

Write
R = L o S o E.

E is exact fiber integration. Its formal finite-volume differential identities are

D E_Phi[A] = E_{Phi,V}[A],
D^2 E_Phi[A,B] = -Cov_{Phi,V}(A,B).

These identities alone DO NOT establish that E maps one of the above infinite-volume uniform Banach spaces to another with bounded derivatives.

S is block identification/rescaling. Its operator norm depends on the chosen polymer metric and weights and must be estimated.

L is localization/extraction. In the old specification it was only named, not constructed as a bounded linear map. Balaban localization is implemented through localized expansions/random-walk/cluster machinery; it has not been identified here with a single fixed bounded linear operator on B_{alpha,mu,p,rho}.

If E is C^2, S is bounded linear, and L is C^2, the correct chain rule is

D R_Phi
 = D L_{S E(Phi)} o S o D E_Phi,

D^2 R_Phi[A,B]
 = D^2 L_{S E(Phi)}
     [S D E_Phi[A], S D E_Phi[B]]
   + D L_{S E(Phi)}
     [S D^2 E_Phi[A,B]].

Only if L is bounded linear does this simplify to
D R = L S D E,
D^2 R = L S D^2 E.

Therefore C_0 in YM-RG-TAYLOR-1 is not instantiated for the full Yang-Mills RG until E/S/L mapping and derivative bounds are supplied.

## 16. Level A versus Level B certificate reduction

The five STEP_j bounds f,H,A,B,C are a FINITE NUMBER of certificate families (Level A).

They are not thereby finite-dimensional computations.

For example
C_j=sup_{Phi in K_j} ||D^2 R_Phi||
is an infinite-dimensional optimization problem unless one further proves a reduction to finite polymer sizes/group integrals plus controlled analytic tails.

The interval verifier checks Level A certificates after they have been rigorously derived. It does not solve Level B.


## 17. Preferred matching norm: pointwise analytic activity norm with decay reserve

The anchored l^1 norm in Section 13 is useful downstream, but it is already stronger than the source statements. For matching, use a pointwise weighted l^infinity norm first.

Fix a compact matching interval for the native marginal parameter
c=1/g^2
(or equivalently a g-interval bounded away from 0 and from the edge of the small-coupling theorem). Choose a common inner analytic domain U^*_{Sigma}(X) contained in every source analyticity domain throughout that interval.

For a sector-indexed activity define

||F||^{pt}_{kappa_*}
 =
 sup_{Sigma,X}
 exp(kappa_* d_Sigma(X))
 ||F_{Sigma,X}||_{H^infty(U^*_{Sigma}(X))}.

Here H^infty carries the sup norm on the complex analytic domain. The product over all (Sigma,X), with weighted l^infinity norm, is Banach because every H^infty(U^*_{Sigma}(X)) is Banach and a weighted l^infinity product of Banach spaces is Banach.

Separate components are normalized by their native amplitudes, e.g. schematically

||R||^{norm}
 = sup exp(kappa_* d) |R_X| / a_R(g),

with a_R(g)=g^{kappa0} when the CMP119 (2.31) bound applies. Analogous component norms are used for E,B,C,R' with the amplitudes actually appearing in their source estimates. No amplitude is invented when it has not been extracted.

Why this norm is preferable:
- a pointwise source estimate |F_X|<=A(g)e^{-kappa d(X)} immediately yields
  ||F||^{pt}_{kappa_*}<=A(g)
  for kappa_*<=kappa;
- characteristic/domain histories remain indices and are not differentiated;
- derivative norms can be recovered on a smaller analytic domain by Cauchy estimates if a uniform analyticity collar is available;
- KP/anchored summability is postponed to YM-NATIVE-TAIL-1, where polymer entropy is paid explicitly.

Thus the logical sequence is

Balaban pointwise analytic bounds
 -> native weighted l^infinity Banach ball
 -> rooted counting + decay reserve
 -> anchored l^1 activity bound
 -> eventual KP/source estimates.

The second arrow is the part directly compatible with the extracted source estimates; the rooted-counting arrow remains a separate theorem.

## 18. Native marginal coordinate and sign convention

CMP119 writes the effective action in the source transcription as

A_k(1/g_k^2,U_k)
 = -A(1/g_k^2,U_k)+E_k+R_k+B_k-mathcalE_k

inside the density exponent.

If our Gibbs convention is exp[-S_eff], the natural positive classical-action coefficient is therefore the source parameter

c_k=1/g_k^2.

CMP119 (2.24) is transcribed as

c_{j-1}(x)
 = c_j(x)+beta_j(g_{j-1}) phi_j(x).

On a homogeneous interior region with phi_j=1, this gives the source-native one-step marginal relation

c_j = c_{j-1}-beta_j(g_{j-1}).

This fixes the qualitative direction of the native coefficient. It does NOT identify c with beta=ell_W(Phi) or with the conventional Wilson beta_W=2N/g^2 without an action-normalization dictionary.

For Iteration 3, c=1/g^2 is therefore the preferred matching coordinate. The ell_W coordinate is suspended for MATCH/STEP_0 until comparability is proved.

## 19. Status of one-step preservation

The source-level theorem shape recorded for CMP119/CMP122 is stronger than a mere finite-volume existence claim: under its recursive coupling inequalities, constant restrictions, and sufficiently-small-coupling assumptions, successive RT/R operations preserve the paper's inductive density form and bounds.

This supports using the native inductive class as the matching state space while g remains in that theorem's small-coupling window.

However it does NOT yet prove that the specific weighted l^infinity norm above is a self-map with an explicit operator constant, because the following have not been fully extracted:
- the complete list of inductive assumptions and constant restrictions;
- a common inner analytic domain U^* over the matching interval;
- exact metric/index dictionaries;
- all component amplitude constants.

Thus one-step preservation is:
KNOWN at the source predicate/class level under small-coupling hypotheses;
NOT YET PROVED as a bounded operator theorem in the new Banach norm.

## 20. Heat-kernel RG candidate versus Balaban RT/R map

Section 5's heat-kernel block map is only an independent exact-RG candidate. It is not identified with Balaban's RT/R operation.

For YM-MATCH-EXTRACT-1 + STEP_0 there are two legitimate choices:

(A) Stay in Balaban's native RT/R map. Then the source inductive estimates are relevant directly, but the exact Banach/operator dictionary must be extracted.

(B) Extract a physical density from Balaban's construction and switch to the heat-kernel RG. Then a new chart-transition theorem must first place that density in the heat-kernel map's input Banach space.

Iteration 3 adopts (A). Therefore no STEP_0 estimate may mix Balaban matching bounds with derivatives of the unrelated heat-kernel candidate without a chart-transition theorem.
